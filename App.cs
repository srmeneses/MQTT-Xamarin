using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MQTTXamarin{

 [DesignTimeVisible(false)]
 public partial class MainPage : ContentPage{
	IMqttClient client;
	SessionState sessionState;

	// Variáveis de configuração
	private string Sala = "119";
	private string CurtainPath = "LIEC/Sala_" + Sala + "/Curtain";
	private string AirPath = "LIEC/Sala_" + Sala + "/Air_Conditioner";
	private string BrokerURL = "broker.mqttdashboard.com";
	private int BrokerPort = 1883;
	private string MQTTClientID = "LiecAplicativoXamarin";

	public MainPage(){
		InitializeComponent();
	}

	private async void Button_ClickedAsync(object sender, EventArgs e){
		var configuration = new MqttConfiguration
		{
			BufferSize = 128 * 1024,
			Port = 1883,
			KeepAliveSecs = 10,
			WaitTimeoutSecs = 2,
			MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
			AllowWildcardsInTopicFilters = true
		};

		Botao1.IsEnabled = false;
		client = await MqttClient.CreateAsync(BrokerURL, BrokerPort);
		sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId:MQTTClientID));
		
		if (client.IsConnected){
			await DisplayAlert("Conexão", "Estado: Conectado", "Ok");
		}
		
		Botao1.IsEnabled = false;
		Botao1.Text = "Conectado";
		Xamarin.Forms.Color xfColor = Xamarin.Forms.Color.FromRgb(0, 255, 0);
		Botao1.BackgroundColor = xfColor;
	}
	//----------------------------------------------------------
	private async void SendMqttMessage(string mensageMqtt, strint topicoMQTT) {
		var message1 = new MqttApplicationMessage(topicoMQTT, Encoding.UTF8.GetBytes(mensageMqtt));
		await client.PublishAsync(message1, MqttQualityOfService.AtMostOnce); //QoS0
	}

	// Cortina ------------------------------------------------------
	private void Button_Clicked_CUPAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			SendMqttMessage("UP", CurtainPath);
		}
	}
	private void Button_Clicked_CDOWNAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			SendMqttMessage("DOWN", CurtainPath);
		}
	}
	private void Button_Clicked_CSTOPAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			SendMqttMessage("STOP", CurtainPath);
		}
	}
	// Ar ------------------------------------------------------
	private void Button_Clicked_ArONAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			SendMqttMessage("ON", AirPath);
		}
	}

	private void Button_Clicked_ArOFFAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			SendMqttMessage("OFF", AirPath);
		}
	}
	private void Button_Clicked_SetPointAsync(object sender, EventArgs e){
		if (client.IsConnected == true){
			string temp = "28";
			temp = setPointText.Text;
			int temperatura = Convert.ToInt32(temp);
			if (temperatura < 18) {
				temp = "18";
				await DisplayAlert("Valor inválido", "A temperatura mínima é de 18 C", "Ok");
			}
			if (temperatura > 28) {
				temp = "28";
				await DisplayAlert("Valor inválido", "A temperatura máxima é de 28 C", "Ok");
			}
			SendMqttMessage(temp, AirPath);
		}
	}

	// Configurações Avançadas ------------------------------------------------------
	private void Button_Clicked_configAvanc(object sender, EventArgs e)
	{
		entryBroker.Text = BrokerURL;
		entryTopico.Text = Sala;
		boxViewDarkTrans.IsVisible = true;
		layoutConfigAvanc.IsVisible = true;
	}

	private void Button_Clicked_okConfigAvanc(object sender, EventArgs e)
	{
		boxViewDarkTrans.IsVisible = false;
		layoutConfigAvanc.IsVisible = false;
		Sala = (string) entryTopico.Text;
		BrokerURL = (string) entryBroker.Text;
		DisplayAlert("Dados alterados!","Verifique o tópico e o endereço do broker","Ok");
	}
	private void Button_Clicked_cancelarConfigAvanc(object sender, EventArgs e){
		boxViewDarkTrans.IsVisible = false;
		layoutConfigAvanc.IsVisible = false;
	}
 }
}