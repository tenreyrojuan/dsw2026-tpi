using System.Linq.Expressions;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Domain.Specifications;

public sealed class DoctorMonthlyAvailabilityRulesSpecification : Specification<AvailabilityRule>
{
    public DoctorMonthlyAvailabilityRulesSpecification(Guid doctorId,
        params string[] includes) 
        : base(rule => rule.DoctorId == doctorId && rule.Month == DateTime.Now.Month && rule.Year == DateTime.Now.Year)
    {
        foreach (var includeExpression in includes)
        {
            AddInclude(includeExpression);
        }
        SetAsNoTracking();
    }
}