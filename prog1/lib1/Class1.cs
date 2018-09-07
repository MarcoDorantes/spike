using System;

namespace lib1
{
    public static class Class1
    {
        public static string f()=>System.Reflection.MethodInfo.GetCurrentMethod().Name;
    }
}