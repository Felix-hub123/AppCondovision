﻿namespace AppCondovision.Data
{
    public interface IEntity
    {
        int Id { get; set; }

        bool WasDeleted { get; set; } 
    }
}
