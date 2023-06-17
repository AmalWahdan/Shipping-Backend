
using Microsoft.IdentityModel.Tokens;
using Shipping.DAL.Data.Models;
using Shipping.DAL.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.DAL.Specifications
{
    public class BranchSpecificaction : BaseSpecification<Branch>
    {

        public BranchSpecificaction(GSpecParams SpecParams)
              : base(x => (string.IsNullOrEmpty(SpecParams.Search) || x.Name.ToLower().Contains(SpecParams.Search))
           && (x.isDeleted == false))
        
        {
            
            AddOrderBy(x => x.Name);
            ApplyPaging(SpecParams.PageSize * (SpecParams.PageIndex -1), SpecParams.PageSize);

            if (!string.IsNullOrEmpty(SpecParams.Sort))
            {
                switch (SpecParams.Sort)
                {
                    case "Desc":
                        AddOrderByDescending(p => p.Name); break;
                    default:
                        AddOrderBy(p => p.Name); break;
                }
            }
        }
        
    }
}
