﻿using Domain.Events.Entities;

namespace Domain.Events.Contracts;

public interface IAmAnEventRepository
{
    public Task Add(Event theEvent);
    public Task Update(Event theEvent);
    public Task<Event?> Get(Guid id);
    public Task<IList<Event>> GetAll();
}