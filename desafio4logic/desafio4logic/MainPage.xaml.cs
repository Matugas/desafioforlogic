using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace desafio4logic
{
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        //Endereço da API que utilizaremos
        RestClient client = new RestClient("https://free.currconv.com/api/v7/");
        //Nossa chave para acessar os recursos da API
        private String apiKey = "b2e89e7541ea8a61ad3b";

        //Array de string para definir os tipos de moedas pedidos no desafio: Dolar, Real, Euro, Franco Suíço, Rupia Indiana, Bitcoin
        private string[] moedas = new string[] { "USD", "BRL", "EUR", "CHF", "INR", "BTC" };


        IList<Currency> all_currencies;
        public MainPage()
        {
            InitializeComponent();
            foreach (String s in moedas)    //Itera sobre o array de moedas e preenche o Picker com os valores
            {
                CurrencyPicker.Items.Add(s);
            }

            this.Load_All_Currencies();
        }

        /**
         * Método serve para carregar as informações das moedas: currencyName, currencySymbol e id
         * Facilitando para mostrar o símbolo na saída
         */
        void Load_All_Currencies()
        {
            RestRequest request = new RestRequest("currencies");
            request.AddParameter("apiKey", apiKey);
            IRestResponse response = client.Get(request);

            string contentJson = response.Content;
            JObject content = JObject.Parse(contentJson);
            IList<JToken> results = content["results"].Children().ToList();
            all_currencies = new List<Currency>();
            foreach (JToken result in results)
            {
                Currency currency = result.First().ToObject<Currency>();
                all_currencies.Add(currency);
            }

            all_currencies = all_currencies.OrderBy(c => c.id).ToList();
        }

        void Mais_Opcoes(object sender, System.EventArgs e)
        {
            CurrencyPicker.Items.Clear();
            foreach (Currency c in all_currencies)
            {
                CurrencyPicker.Items.Add(c.id);
            }
        }

        /**
         * Método que converte o valor digitado no Entry e da moeda selecionada no Picker
         * para as outras moedas pedidas no desafio e mostra em um Label.
         */
        void Convert_Currency(object sender, System.EventArgs e)
        {
            double valorEntrada = Convert.ToDouble(MoedaEntry.Text);        //Busca e salva o valor inserido no Entry
            string moedaEntrada = CurrencyPicker.SelectedItem.ToString();   //Busca e salva a moeda selecionada no Picker
            List<String> moedasSaida = new List<String>();
            foreach (string moeda in this.moedas)   //Monta a query das conversoes para enviar como parametro para a API
            {
                moedasSaida.Add(moedaEntrada + "_" + moeda);
            }

            RestRequest request = new RestRequest("convert");
            request.AddParameter("q", String.Join(",", moedasSaida));
            request.AddParameter("compact", "ultra");
            request.AddParameter("apiKey", apiKey);
            IRestResponse response = client.Get(request);

            JObject content = JObject.Parse(response.Content);

            List<String> convertidos = new List<String>();
            foreach (String s in moedas)    //Itera sobre a array das moedas, faz o calculo, pega o simbolo de cada moeda e mostra na Label
            {
                if (moedaEntrada != s)      //Verifica para não converter para a mesma moeda selecionada
                {
                    Currency currency = this.all_currencies.ToList().Find(c => c.id == s);
                    string simbolo = currency.symbol;
                    string texto = moedaEntrada + "_" + s;
                    double taxaCambio = Convert.ToDouble(content[texto]);
                    convertidos.Add(String.Format("{0} = {1}", simbolo, (valorEntrada * taxaCambio)));
                }
            }

            ConvertidoLabel.Text = String.Join("\n", convertidos);
        }
    }
}
