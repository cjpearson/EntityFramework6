﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Migrations.Infrastructure
{
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations.Infrastructure.FunctionsModel;
    using System.Linq;
    using Xunit;

    public class ModificationCommandTreeGeneratorTests
    {
        public class WorldContext : DbContext
        {
            static WorldContext()
            {
                Database.SetInitializer<WorldContext>(null);
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Thing>()
                    .MapToStoredProcedures();
            }
        }

        public class WorldContext_Fk : WorldContext
        {
            static WorldContext_Fk()
            {
                Database.SetInitializer<WorldContext_Fk>(null);
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder
                    .Entity<Thing>()
                    .HasRequired(t => t.Container)
                    .WithMany()
                    .HasForeignKey(t => t.ContainerFk);
            }
        }

        public class Thing
        {
            public int Id { get; set; }
            public Thing Container { get; set; }
            public int ContainerFk { get; set; }
        }

        [Fact]
        public void Can_generate_dynamic_insert_command_trees_for_self_ref_fk()
        {
            DbModel model;

            using (var context = new WorldContext_Fk())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateInsert(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_update_command_trees_for_self_ref_fk()
        {
            DbModel model;

            using (var context = new WorldContext_Fk())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateUpdate(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_delete_command_trees_for_self_ref_fk()
        {
            DbModel model;

            using (var context = new WorldContext_Fk())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateDelete(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_insert_command_trees_for_self_ref_ia()
        {
            DbModel model;

            using (var context = new WorldContext())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateInsert(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_update_command_trees_for_self_ref_ia()
        {
            DbModel model;

            using (var context = new WorldContext())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateUpdate(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_delete_command_trees_for_self_ref_ia()
        {
            DbModel model;

            using (var context = new WorldContext())
            {
                model
                    = context
                        .InternalContext
                        .CodeFirstModel
                        .CachedModelBuilder
                        .BuildDynamicUpdateModel(ProviderRegistry.Sql2008_ProviderInfo);
            }

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateDelete(GetType().Namespace + ".Thing")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());
        }

        [Fact]
        public void Can_generate_dynamic_insert_command_trees_for_many_to_many_association()
        {
            var model = TestContext.CreateDynamicUpdateModel();

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateAssociationInsert(GetType().Namespace + ".FunctionsModel.OrderThing_Orders")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());

            var commandTree = commandTrees.First();

            Assert.Equal(5, commandTree.SetClauses.Count);
            Assert.Equal("OrderThingOrder", commandTree.Target.VariableType.EdmType.Name);
            Assert.Null(commandTree.Returning);
        }

        [Fact]
        public void Can_generate_dynamic_delete_command_trees_for_many_to_many_association()
        {
            var model = TestContext.CreateDynamicUpdateModel();

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateAssociationDelete(GetType().Namespace + ".FunctionsModel.OrderThing_Orders")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());

            var commandTree = commandTrees.First();

            Assert.Equal("OrderThingOrder", commandTree.Target.VariableType.EdmType.Name);
            Assert.NotNull(commandTree.Predicate);
        }

        [Fact]
        public void Can_generate_dynamic_insert_command_trees()
        {
            var model = TestContext.CreateDynamicUpdateModel();

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateInsert(GetType().Namespace + ".FunctionsModel.SpecialOrder")
                    .ToList();

            Assert.Equal(2, commandTrees.Count());

            var commandTree = commandTrees.First();

            Assert.Equal(8, commandTree.SetClauses.Count);
            Assert.Equal("Order", commandTree.Target.VariableType.EdmType.Name);
            Assert.NotNull(commandTree.Returning);

            commandTree = commandTrees.Last();

            Assert.Equal(8, commandTree.SetClauses.Count);
            Assert.NotNull(commandTree.Returning);
            Assert.Equal("special_orders", commandTree.Target.VariableType.EdmType.Name);

            commandTrees
                = commandTreeGenerator
                    .GenerateInsert(GetType().Namespace + ".FunctionsModel.Customer")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());

            commandTree = commandTrees.Single();

            Assert.Equal(1, commandTree.SetClauses.Count);
            Assert.Equal("Customer", commandTree.Target.VariableType.EdmType.Name);
            Assert.NotNull(commandTree.Returning);
        }

        [Fact]
        public void Can_generate_dynamic_update_command_trees()
        {
            var model = TestContext.CreateDynamicUpdateModel();

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateUpdate(GetType().Namespace + ".FunctionsModel.SpecialOrder")
                    .ToList();

            Assert.Equal(2, commandTrees.Count());

            var commandTree = commandTrees.First();

            Assert.Equal(6, commandTree.SetClauses.Count);
            Assert.NotNull(commandTree.Predicate);
            Assert.NotNull(commandTree.Returning);
            Assert.Equal("Order", commandTree.Target.VariableType.EdmType.Name);

            commandTree = commandTrees.Last();

            Assert.Equal(4, commandTree.SetClauses.Count);
            Assert.NotNull(commandTree.Predicate);
            Assert.NotNull(commandTree.Returning);
            Assert.Equal("special_orders", commandTree.Target.VariableType.EdmType.Name);

            commandTrees
                = commandTreeGenerator
                    .GenerateUpdate(GetType().Namespace + ".FunctionsModel.Customer")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());

            commandTree = commandTrees.Single();

            Assert.Equal(1, commandTree.SetClauses.Count);
            Assert.Equal("Customer", commandTree.Target.VariableType.EdmType.Name);
            Assert.Null(commandTree.Returning);
        }

        [Fact]
        public void Can_generate_dynamic_delete_command_trees()
        {
            var model = TestContext.CreateDynamicUpdateModel();

            var commandTreeGenerator
                = new ModificationCommandTreeGenerator(model);

            var commandTrees
                = commandTreeGenerator
                    .GenerateDelete(GetType().Namespace + ".FunctionsModel.SpecialOrder")
                    .ToList();

            Assert.Equal(2, commandTrees.Count());

            var commandTree = commandTrees.First();

            Assert.NotNull(commandTree.Predicate);
            Assert.Equal("special_orders", commandTree.Target.VariableType.EdmType.Name);

            commandTree = commandTrees.Last();

            Assert.NotNull(commandTree.Predicate);
            Assert.Equal("Order", commandTree.Target.VariableType.EdmType.Name);

            commandTrees
                = commandTreeGenerator
                    .GenerateDelete(GetType().Namespace + ".FunctionsModel.Customer")
                    .ToList();

            Assert.Equal(1, commandTrees.Count());

            commandTree = commandTrees.Single();

            Assert.NotNull(commandTree.Predicate);
            Assert.Equal("Customer", commandTree.Target.VariableType.EdmType.Name);
        }
    }
}
