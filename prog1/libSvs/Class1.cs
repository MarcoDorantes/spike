using System;

namespace libSvs
{
    public static class Class1
    {
        public static string f()=>System.Reflection.MethodInfo.GetCurrentMethod().Name;
    }
}