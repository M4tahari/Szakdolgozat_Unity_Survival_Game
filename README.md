# Szakdolgozat_Unity_Survival_Game

irányítás:
-A/bal nyíl : mozgás balra
-D/jobb nyíl : mozgás jobbra
-space: ugrás
-shift nyomva tartva: sprint 
-I : inventory

A játék egy menüben kezdődik, ahol az új játék gombra nyomva egy új világ generálódik le.
A világ betöltése egy már korábban létrehozott világot tölt be, ha nincsen már lementett világ, akkor egy új világ jön létre
A beállítások gombnak még egyenlőre nincsen funkciója, egy külön menüpontot tervezek készíteni, ahol a világ generálás tulajdnoságai lesznek módosíthatóak.
A kilépés gombbal bezáródik az alkalmazás.

A játék pályája blokkokból áll, ezeket ki lehet ütni, hogyha a bal egérgombot nyomva tartjuk felettük. Ez után az adott blokk az inventoryba kerül, ahol ha a bal egérgombot nyomva tartjuk a tárgy felett, akkor áthelyezhető másik helyre az inventoryban. Emellett ha jobb klikkelünk az adott tárgyra, és az lehelyezhető, akkor a blokk követni fogja a kurzort, jobb klikkel visszakerül az adott helyre az inventoryban, bal klikkel pedig lehelyezhető a pályára.

A játékállás lementésére egy json szerializálást és deszerializálást alkalmazok, azonban a mentés funkción még finomítani fogok. Emellett a következő funkciók, amiket meg szeretnék valósítani, azok az ellenfelek, napszak változása, a világgenerálás további optimalizálása, animációk.
