using EncuestasApi.Models.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace EncuestasApi.Hubs
{
    public class HubEncuesta: Hub
    {
        private static int Count = 0;
        public override Task OnConnectedAsync()
        {
            Count++;
            if (Count > 0)
            {

                base.OnConnectedAsync();
                Clients.All.SendAsync("updateCount", Count - 1);
                return Task.CompletedTask;
            }
            else
            {
                base.OnConnectedAsync();
                Clients.All.SendAsync("updateCount", Count - 1);
                return Task.CompletedTask;
            }

        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Count--;
            base.OnDisconnectedAsync(exception);
            Clients.All.SendAsync("updateCount", Count);
            return Task.CompletedTask;
        }

        public async Task MandarAplicacion(EncuestaAplicadaDto dto)
        {
            await Clients.All.SendAsync("Recibir aplicación", dto);
        }
    }
}
