using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsAdmin { get; set; }
    }

    public class UserResult
    {
        public int Count { get; set; }

        public List<UserDto> Users { get; set; }
    }
}
