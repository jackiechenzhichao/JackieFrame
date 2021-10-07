using UnityEngine;
using System.Collections;
using AntiCheat.ObscuredTypes;
using AntiCheat.Detectors;

namespace fs
{
    /// <summary>
    /// 防作弊
    /// @author hannibal
    /// @time 2016-4-12
    /// </summary>
    public class AntiCheatManager : DnotMonoSingleton<AntiCheatManager>
    {
        void Start()
        {
            //内存修改
            ObscuredBool.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredByte.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredChar.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredDecimal.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredDouble.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredFloat.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredInt.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredLong.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredQuaternion.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredSByte.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredShort.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredString.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredUInt.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredULong.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredUShort.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredVector2.onCheatingDetected = OnMemoryCheatingDetected;
            ObscuredVector3.onCheatingDetected = OnMemoryCheatingDetected;

            //加速
            //SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1, 5);

            //代码注入
            //InjectionDetector.StartDetection(OnInjectionDetected);
        }

        /// <summary>
        /// 内存修改
        /// </summary>
        private void OnMemoryCheatingDetected()
        {
            Debuger.LogError("OnMemoryCheatingDetected");
        }
        /// <summary>
        /// 加速
        /// </summary>
        private void OnSpeedHackDetected()
        {
            //Debuger.LogError("OnSpeedHackDetected");
            //Application.Quit();
        }

        /// <summary>
        /// 代码注入
        /// </summary>
        private void OnInjectionDetected()
        {
            //Debuger.LogError("OnInjectionDetected!");
            //Application.Quit();
        }
    }
}