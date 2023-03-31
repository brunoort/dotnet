using Humanizer.Bytes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        //contador de requisições
        private int Rc = 0;
        //informações de data da API TimeAPI
        private string R;
        private string K;
        private List<int> On = new List<int>();

        public IActionResult Index(int concurrency)
        {
            try
            {
                // Verifica se a API tem recursos para atender a requisição
                if (!Program.CheckIfThereIsThreadAvailable())
                    return BadRequest(Messages.THREAD_IS_NO_THREAD_AVAILABLE);
                else
                {
                    Rc++;
                    R = new HttpClient().GetStringAsync(Program.API_ADDRESS).Result;

                    // Calcula e gera a chave
                    for (int i = 0; i < 4096; i++)
                        K = string.Concat(K, (DateTime.Parse(R).Day) * i * new Random().Next(100, 9999));

                    // Obtem os números ímpares gerados na chave
                    foreach (var c in K.ToArray())
                        if (int.Parse(c.ToString()) % 2 != 0)
                            On.Add(int.Parse(c.ToString()));


                    // Aplica valores para View
                    ViewData["DateTimeNow"] = R;
                    ViewData["Key"] = K;
                    ViewData["Sum"] = On.Sum(x => x);
                    ViewData["VirtualMachines"] = Program.NUMBER_OF_VIRTUAL_MACHINES;
                    ViewData["RequestsCounter"] = Rc;
                    // Aplica informações de memória para View
                    ViewData["gc0"] = GC.CollectionCount(0);
                    ViewData["gc1"] = GC.CollectionCount(1);
                    ViewData["gc2"] = GC.CollectionCount(2);
                    ViewData["currentMemory"] = ByteSize.FromBytes(GC.GetTotalMemory(false)).ToString();
                    ViewData["privateBytes"] = ByteSize.FromBytes(Process.GetCurrentProcess().WorkingSet64);
                }
            }
            catch
            {

            }
            // Retorna View
            return View();
        }

        public IActionResult Information()
        {
            return View();
        }
    }
}