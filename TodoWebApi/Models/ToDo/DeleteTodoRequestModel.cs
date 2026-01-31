using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoWebApi.Models.ToDo
{
    public class DeleteTodoRequestModel
    {
        public int TodoId { get; set; }
        public int UserId { get; set; }
    }
}