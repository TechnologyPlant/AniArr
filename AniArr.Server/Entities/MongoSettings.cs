﻿namespace AniArr.Server.Entities;

public class MongoSettings
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Database { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
