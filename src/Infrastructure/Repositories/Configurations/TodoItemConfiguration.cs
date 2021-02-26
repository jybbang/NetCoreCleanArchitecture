using DaprCleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DaprCleanArchitecture.Infrastructure.Repositories.Configurations
{
    public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.ToTable("TB_TODOITEM");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Detail);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}