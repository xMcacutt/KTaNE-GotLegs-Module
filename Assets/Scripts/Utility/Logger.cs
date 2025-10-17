using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KModkit
{
    public static class Logger
    {
        public static void Log(this string message, Module module)
        {
            Debug.LogFormat("[{0} #{1}] {2}", module.module.ModuleDisplayName, module.moduleId, message);
        }
    }
}