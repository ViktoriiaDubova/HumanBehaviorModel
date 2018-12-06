using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HBM.Web.ViewModels
{
    public class AssignRoleViewModel
    {
        public int UserId { get; set; }
        public int? SelectedRoleId { get; set; }
        public string UserName { get; set; }
        public SelectList Roles { get; set; }
    }
}