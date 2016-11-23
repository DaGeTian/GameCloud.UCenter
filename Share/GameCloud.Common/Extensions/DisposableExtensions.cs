﻿using System;

namespace GameCloud.Common.Extensions
{
    public static class DisposableExtensions
    {
        public static void DisposeOnException(this IDisposable obj, Action action)
        {
            bool success = false;
            try
            {
                action();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    obj.Dispose();
                }
            }
        }
    }
}