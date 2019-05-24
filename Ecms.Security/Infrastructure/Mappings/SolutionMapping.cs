﻿using ANMappings;
using Ecms.Security.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Security.Infrastructure.Mappings
{
    public class SolutionMapping: Mapping<Solution>
    {
        public const string TABLE_NAME = "solutions";
        public const string COLUMN_ID = "solution_id";
        public const string COLUMN_NAME = "solution_name";
        public const string COLUMN_DESCRIPTION = "solution_description";
        public const string COLUMN_CODE = "solution_code";
        public const string COLUMN_PROPERTIES = "solution_properties";
        public const string COLUMN_CREATED_TIME = "solution_created_time";
        public const string COLUMN_CREATED_BY = "solution_created_by";
        public const string COLUMN_CREATED_IP_ADDRESS = "solution_created_ip_address";
        public const string COLUMN_MODIFIED_TIME = "solution_modified_time";
        public const string COLUMN_MODIFIED_BY = "solution_modified_by";
        public const string COLUMN_MODIFIED_IP_ADDRESS = "solution_modified_ip_address";
        public const string COLUMN_LOG_ID = "solution_log_id";
        public const string COLUMN_IS_ACTIVE = "solution_is_active";
        public const string COLUMN_IS_REMOVED = "solution_is_removed";

        public SolutionMapping()
        {
            Named(TABLE_NAME);
            Identity(x => x.Id).Named(COLUMN_ID);
            Map(x => x.Name).Named(COLUMN_NAME).HasMaxLength(50);
            Map(x => x.Description).Named(COLUMN_DESCRIPTION).HasMaxLength(512).Nullable();
            Map(x => x.Code).Named(COLUMN_CODE).HasMaxLength(50).Nullable();
            Map(x => x.Properties).Named(COLUMN_PROPERTIES).Nullable();
            Map(x => x.CreatedTime).Named(COLUMN_CREATED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.CreatedBy).Named(COLUMN_CREATED_BY).HasMaxLength(50).Nullable();
            Map(x => x.CreatedIpAddress).Named(COLUMN_CREATED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.ModifiedTime).Named(COLUMN_MODIFIED_TIME).Nullable().DefaultCurrentUtcDateTime();
            Map(x => x.ModifiedBy).Named(COLUMN_MODIFIED_BY).HasMaxLength(50).Nullable();
            Map(x => x.ModifiedIpAddress).Named(COLUMN_MODIFIED_IP_ADDRESS).HasMaxLength(40).Nullable();
            Map(x => x.LogId).Named(COLUMN_LOG_ID).Nullable();
            Map(x => x.IsActive).Named(COLUMN_IS_ACTIVE).Default(true);
            Map(x => x.IsRemoved).Named(COLUMN_IS_REMOVED).Default(false);
        }
    }
}
