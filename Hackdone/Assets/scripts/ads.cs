using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class ads : MonoBehaviour
{

	private string appID;
	private string bannerID;
	private string interstitialID;
	public bool showIinterstitial;
	// ca-app-pub-3940256099942544/6300978111 - тестовый
	private BannerView bannerView;
	private InterstitialAd interstitial;
	//private string isLoaded, isFailed;



	void Start ( )
	{
		 appID = "ca-app-pub-7776748974347263~5097264269";
		 bannerID = "ca-app-pub-7776748974347263/3229648826";
		 interstitialID = "ca-app-pub-7776748974347263/9136226400";
		// инициализация рекламного SDK
		MobileAds.Initialize (appID);

		this.RequestBanner ( ); // Банер показывается всегда, после загрузки это функции
		this.LoadingInterstitial ( );
  }

	void Update ( )
	{
        // Показа межстраничного обявления
		if (showIinterstitial)
		{
			if (interstitial.IsLoaded()) {
				interstitial.Show();
				showIinterstitial = false;
			}
		}
	}

	public void LoadingInterstitial ( )
	{
    interstitial = new InterstitialAd(interstitialID); // Инициализация межстранички
    // Добавлемя обработку событий с межстраничным объявлением
    this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
    this.interstitial.OnAdClosed += HandleOnAdClosed;
    this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

    AdRequest request = new AdRequest.Builder().Build();
    interstitial.LoadAd(request);
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		interstitial.Destroy ();
		LoadingInterstitial ( );
	}

	public void HandleOnAdClosed(object sender, System.EventArgs args)
	{
		interstitial.Destroy ();
		LoadingInterstitial ( );
	}

	public void HandleOnAdLeavingApplication(object sender, System.EventArgs args)
	{
		interstitial.Destroy ();
		LoadingInterstitial ( );
	}

  private void RequestBanner()
  {
    // загрузка банерной рекламы
    bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);
		AdRequest request = new AdRequest.Builder().Build();
		bannerView.LoadAd(request);
  }
}
