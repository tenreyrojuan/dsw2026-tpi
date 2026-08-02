using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Helpers;
using Dsw2026Tpi.CrossCutting.Identity;
using Dsw2026Tpi.CrossCutting.Resources;
using Dsw2026Tpi.Data.Identity;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Dsw2026Tpi.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISignInService _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IPersistence _persistence;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
        ISignInService signInManager,
        RoleManager<IdentityRole> roleManager,
        JwtService jwtService,
        ILogger<AuthenticationService> logger,
        IPersistence persistence)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _logger = logger;
        _persistence = persistence;
    }
    public async Task<LoginAdminModel.Response> LoginAdmin(LoginAdminModel.Request request)
    {
        if (!request.Email.IsEmailValid()) 
            throw new AuthenticationException()
                .WithDetail(nameof(request.Email), Issue.INVALID_EMAIL);

        var user = await _userManager.FindByEmailAsync(request.Email) 
            ?? throw new AuthenticationException()
                .WithDetail(nameof(request.Email), Issue.EMAIL_ERROR);

        var result = await _signInManager.CheckPassword(user, request.Password);

        if (!result)
        {
            _logger.LogError("Intento de login fallido para: {Email}", request.Email);
            throw new AuthenticationException()
                .WithDetail(nameof(result), Issue.PASSWORD_ERROR);
        }

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        var token  = _jwtService.GenerateToken(user.UserName!, role);

        return new LoginAdminModel.Response(
            token,
            role
        );
    }

    public async Task<LoginPatientModel.Response> LoginPatient(LoginPatientModel.Request request)
    {
        var role = Roles.Patient;

        if (!request.Email.IsEmailValid())
            throw new AuthenticationException()
                .WithDetail(nameof(request.Email), Issue.INVALID_EMAIL);

        var dni = request.Dni.ToString();

        if (!dni.IsDniValid())
            throw new AuthenticationException()
                .WithDetail(nameof(dni), Issue.INVALID_DNI);

        var user = await _userManager.FindByEmailAsync(request.Email);

        // Usuario Existe
        if (user != null)
        {

            var patient = await _persistence.First<Patient>(p => p.UserId == Guid.Parse(user.Id))
                ?? throw new Exception();

            if (patient.Dni != dni)
            {
                throw new AuthenticationException()
                    .WithDetail(nameof(dni), Issue.PASSWORD_ERROR);
            }
            var token = _jwtService.GenerateToken(user.UserName!, role);
            return new LoginPatientModel.Response(
                token,
                role
            );
        }
        // Usuario Nuevo
        var userExist = await _persistence.First<Patient>(p => p.Dni == dni);
        if (userExist != null)
            throw new ConflictException()
                .WithDetail(nameof(dni), Issue.DUPLICATE_DNI);

        user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("Error al crear el paciente con Email: {Email}", request.Email);
            throw new ConflictException()
                .WithDetail(result.Errors.Select(e => (e.Code, e.Description)));
        }

        var patientNew = new Patient(dni, request.Email, Guid.Parse(user.Id));

        var newPatient = await _persistence.Add<Patient>(patientNew);

        if (newPatient is null)
            _logger.LogError("Error al crear el paciente con Email: {Email}", request.Email);

        _ = await _userManager.AddToRoleAsync(user, Roles.Patient);

        var tokenNew = _jwtService.GenerateToken(user.UserName!, role);
        return new LoginPatientModel.Response(tokenNew, role);
    }

    public async Task<RegisterModel.Response> Register(RegisterModel.Request request)
    {
        if (!request.Email.IsEmailValid()) 
            throw new ValidationException()
                .WithDetail(nameof(request.Email), Issue.INVALID_EMAIL);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) 
            throw new ConflictException()
                .WithDetail(result.Errors.Select(e => (e.Code, e.Description)));

        _ = await _userManager.AddToRoleAsync(user, Roles.Administrator);

        _logger.LogInformation("Usuario registrado: {Email}", request.Email);

        return new RegisterModel.Response(request.Email);
    }
}
