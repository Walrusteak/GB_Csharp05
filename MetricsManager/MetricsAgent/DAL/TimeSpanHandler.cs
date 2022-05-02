﻿using Dapper;
using System.Data;
using System;
namespace MetricsAgent.DAL
{
    public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
    {
        public override TimeSpan Parse(object value) => TimeSpan.FromSeconds((long)value);

        public override void SetValue(IDbDataParameter parameter, TimeSpan value) => parameter.Value = value;
    }
}
