using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Models.Interface
{
   public interface IDateTracking
    {
        DateTime CreatedTime { get; set; }
        DateTime? ModifiedTime { get; set; }
    }
}
