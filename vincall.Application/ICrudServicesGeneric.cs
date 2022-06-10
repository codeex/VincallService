using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application
{
    public interface ICrudServices<TContext> : ICrudServices where TContext : DbContext { }
}
