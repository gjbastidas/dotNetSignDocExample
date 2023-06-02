using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotNetSignDocExample.Services.EnvironmentService
{
  public interface IEnvironmentService
  {
    EnvironmentVariables GetEnvironmentVariables();
  }
}