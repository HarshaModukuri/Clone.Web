using OktaClone.Web.Models;
using System.Collections.Generic;

namespace OktaClone.Web.ViewModels
{
    public class DashboardViewModel
    {
        public List<Application>? AssignedApplications { get; set; }
        public List<Application>? UnassignedApplications { get; set; }
    }
}
