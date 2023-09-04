using Microsoft.EntityFrameworkCore;
using Models;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AuditableRealEstateDbContext : DbContext
    {
        public DbSet<Audit> Audits { get; set; }
        public AuditableRealEstateDbContext() {}
        public AuditableRealEstateDbContext(DbContextOptions<RealEStateDbContext> options) : base(options)
        {
        }

        public async Task<int> SaveChangesAsync(int userId)
        {
            var auditEntries = OnBeforeSaveChanges(userId);
            var saveResult = await base.SaveChangesAsync();
            if(auditEntries != null && auditEntries.Count > 0) {
                await OnAfterSaveChanges(auditEntries);
            }


            return saveResult;
        }

        private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    } else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                Audits.Add(auditEntry.ToAudit());
            }

            await SaveChangesAsync();
        }

        private List<AuditEntry> OnBeforeSaveChanges(int userId)
        {
            var entries = ChangeTracker.Entries().Where(q => q.State == EntityState.Added || q.State == EntityState.Modified || q.State == EntityState.Deleted);
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in entries)
            {
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString()
                };

                var auditedObject = (BaseEntity) entry.Entity;

                if(entry.State == EntityState.Added)
                {
                    auditedObject.CreatedAt = DateTime.Now;
                    auditedObject.CreatedBy = userId;
                }

                if(entry.State == EntityState.Modified)
                {
                    auditedObject.UpdatedAt = DateTime.Now;
                    auditedObject.UpdatedBy = userId;
                }

                foreach (var property in entry.Properties)
                {
                    // 1) Check if the property is temporary
                    // A temporary property when marked as "true" means that the entry is about to be inserted in Database. Without having an Id assigned toit,
                    // since Id columns are, by default, IDENTITY(1,1)
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    // 2) Check if the property is Primary Key
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    //3) Check New and/or Old values for this property before save
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                            }
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            // Checking for tracked entities that are not being ADDED in the database, since actions
            // such as MODIFIED and DELETED have already information about the ID rather than action ADDED whose 
            // Id is still about to be created with IDENTITY(1,1) database behavior
            foreach (var pendingAuditEntry in auditEntries.Where(ae => ae.HasTemporaryProperties == false))
            {
                Audits.Add(pendingAuditEntry.ToAudit());
            }

            return auditEntries.Where(ae => ae.HasTemporaryProperties).ToList();
        }
    }
}
