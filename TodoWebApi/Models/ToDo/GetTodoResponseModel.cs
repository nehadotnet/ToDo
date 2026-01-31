using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.ToDo
{
    public class GetTodoResponseModel
    {
        public int TodoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public int RemainingDays { get; set; }
    }
}