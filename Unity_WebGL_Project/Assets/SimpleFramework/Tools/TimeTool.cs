//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;


//public static class TimeTool
//{
//    //------------------------------------------------------------------------------------------
//    public static UInt64 GetNowTimeStamp()
//    {
//        System.DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
//        return GetTimeStampFromUTCTime(utcTime);
//    }

//    public static UInt64 GetTimeStampFromLocalTime(DateTime nLocalTime)
//	{
//		System.DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(nLocalTime, TimeZoneInfo.Local);
//		return GetTimeStampFromUTCTime(utcTime);
//	}

//	public static UInt64 GetTimeStampFromUTCTime(DateTime utcTime)
//	{
//		TimeSpan ts = utcTime - new DateTime(1970, 1, 1, 0, 0, 0);
//		return (UInt64)ts.TotalSeconds;
//	}

//	public static DateTime GetUTCTimeFromTimeStamp(UInt64 nTimeStamp)
//	{
//		DateTime dateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);
//		return dateTimeStart.AddSeconds(nTimeStamp);
//	}

//	public static DateTime GetLocalTimeFromTimeStamp(UInt64 mTimeStamp)
//	{
//		DateTime utcTime = GetUTCTimeFromTimeStamp(mTimeStamp);
//		return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
//	}

//	public static DateTime GetLocalTimeFromUTCTime(DateTime utcTime)
//	{
//		return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
//	}

//	public static DateTime GetUtcTimeFromLocalTime(DateTime LocalTime)
//	{
//		return TimeZoneInfo.ConvertTimeToUtc(LocalTime, TimeZoneInfo.Utc);
//	}
//}

