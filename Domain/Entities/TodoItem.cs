using DotNnetcoreCleanArchitecture.Domain.Abstracts;
using DotNnetcoreCleanArchitecture.Domain.Events;
using DotNnetcoreCleanArchitecture.Domain.ValueObjects;
using System;

namespace DotNnetcoreCleanArchitecture.Domain.Entities
{
    public class TodoItem : EntityHasDomainEvent, IAuditableEntity
    {
        private Detail _detail = new Detail("Title", "Description", "White");
        private bool _done;

        public Detail Detail
        {
            get => _detail;
            set => PropertyChanged(ref _detail, value);
        }

        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    Commit(new TodoItemCompletedEvent(this, Version, this));
                }
                _done = value;
            }
        }
        public string CreateUserId { get; set; }

        public string UpdateUserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}