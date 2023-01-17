using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIRcat_Control;

namespace psc.mircat.coms.sdk.test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" ------------------------------------");
                Console.WriteLine(" PSC test of MIRcat SDK communication");
                Console.WriteLine(" ------------------------------------");

                Console.WriteLine("\n[Information] initiating connection...");
                MIRcatSDK.MIRcatSDK_Initialize().ThrowOnError();

                bool isConnected = false;
                MIRcatSDK.MIRcatSDK_IsConnectedToLaser(ref isConnected).ThrowOnError();
                Console.WriteLine($"\n[Information] connection {(isConnected ? "successful" : "** FAIL **")}");

                Console.WriteLine($"\n[Information] invoking MIRcatSDK_IsInterlockedStatusSet method...");
                bool interlockSet = false;
                MIRcatSDK.MIRcatSDK_IsInterlockedStatusSet(ref interlockSet).ThrowOnError();
                Console.WriteLine($"received: {interlockSet}\n");


                Console.WriteLine("\n[Information] disconnecting...");
                MIRcatSDK.MIRcatSDK_DeInitialize().ThrowOnError();
                MIRcatSDK.MIRcatSDK_IsConnectedToLaser(ref isConnected);
                Console.WriteLine($"\n[Information] disconnection {(!isConnected ? "successful" : "** FAIL **")}");

                Console.WriteLine("\n\n** TEST PASSED **\n");


                Console.WriteLine("\n[Information] Exiting normally...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                Console.WriteLine("\n\n** TEST FAILED **\n");
            }

            Console.WriteLine("\nPress any key to close...");
            Console.ReadKey();
        }
    }

    public static class Extensions
    {
        public static bool IsError(this SDKConstant result) => SDKConstant.MIRcatSDK_RET_SUCCESS != result;

        public static void ThrowOnError(this SDKConstant result)
        {
            if (result.IsError())
            {
                throw new Exception($"'{result} | {MIRcatSDK.MIRcatSDK_GetErrorDesc(result)}");
            }
        }
    }
}
