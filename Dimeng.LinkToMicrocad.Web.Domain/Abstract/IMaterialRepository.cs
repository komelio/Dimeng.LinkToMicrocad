using Dimeng.LinkToMicrocad.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimeng.LinkToMicrocad.Web.Domain.Abstract
{
    public interface IMaterialRepository
    {
        IEnumerable<Material> Materials { get; }
        int Add(Material material);
        void AddRange(IEnumerable<Material> materials);
        void ApplyModel(Material material);
    }
}
