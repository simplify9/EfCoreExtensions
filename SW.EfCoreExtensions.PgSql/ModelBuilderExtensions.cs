﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SW.EfCoreExtensions
{
    public static class ModelBuilderExtensions
    {
        //public static ModelBuilder EntityFeatures<TEntity>(this ModelBuilder modelBuilder, Action<EntityFeaturesBuilder<TEntity>> builder) where TEntity : class
        //{
        //    var entityFeaturesBuilder = new EntityFeaturesBuilder<TEntity>(modelBuilder);
        //    builder.Invoke(entityFeaturesBuilder);
        //    return modelBuilder;
        //}

        public static ModelBuilder CommonProperties(this ModelBuilder modelBuilder, Action<CommonPropertiesBuilder> builder) 
        {
            var commonPropertiesBuilder = new CommonPropertiesBuilder(modelBuilder);
            builder.Invoke(commonPropertiesBuilder);
            return modelBuilder;
        }
    }
}
