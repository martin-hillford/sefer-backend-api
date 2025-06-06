global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.DependencyInjection;
global using Sefer.Backend.Api.Chat.Models;
global using Sefer.Backend.Api.Chat.Views;
global using Sefer.Backend.Api.Data.JsonViews;
global using Sefer.Backend.Api.Data.Models.Constants;
global using Sefer.Backend.Api.Data.Models.Users;
global using Sefer.Backend.Api.Data.Models.Users.Chat;
global using Sefer.Backend.Api.Data.Requests.Channels;
global using Sefer.Backend.Api.Data.Requests.ChatMessages;
global using Sefer.Backend.Api.Data.Requests.Users;
global using Sefer.Backend.Api.Services.Notifications;
global using Sefer.Backend.Support.Lib.Mediator;
global using Sefer.Backend.Authentication.Lib;