using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FamilyGroup
{
    public interface IDisplayChild
    {
        int No { get; }
        string Id { get; }
        string Name { get; }
        string Sex { get; }
        string BDate { get; }
        string DDate { get; }
        string BPlace { get; }
        string DPlace { get; }
        string MDate { get; }
        string MPlace { get; }
        string MSpouse { get; }
    }
}
