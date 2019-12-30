# FoundryReports
Meine Abgabe für https://www.it-talents.de/foerderung/code-competition/zf-code-competition-11-2019

# Benutzung:
- Release herunterladen von https://github.com/BerndNK/FoundryReports/releases

- FoundryReports.exe starten. Hier befinden sich jetzt 3 Reiter "Products", "Casting Cell" und "Data".
![Startseite](https://github.com/BerndNK/FoundryReports/blob/master/Documentation/Empty.PNG?raw=true)

- Als allererstes muss man das Tool mit Daten befüllen, das kann man im Reiter "Data" tun. Hier kann man mit "+ New item" jeweils neue Formen, Produkte und Kunden anlegen. Am einfachsten ist es aber mit dem jeweiligen "Import..." Button. Hier kann man die jeweiligen Daten einlesen lassen. Für die Code-Competition liegen die Daten bereit im Ordner "FoundryReports/ExampleData/". 
![Daten import](https://github.com/BerndNK/FoundryReports/blob/master/Documentation/Import.PNG?raw=true)
"Molds.csv" für Formen (Molds), "Products.csv" für Produkte (Products) und "CustomerX.csv" für den Kunden aus dem Beispiel (Customer).
![Daten sind importiert](https://github.com/BerndNK/FoundryReports/blob/master/Documentation/DataIsImported.PNG?raw=true)
Wichtig ist, dass nach jeder Änderung die persistiert werden soll, der Button "Persist changes" gedrückt werden muss. Sollte dies nicht passieren, sind die geänderten Daten nach einem Neustart nicht mehr vorhanden.

- Jetzt kann man sich diese Daten visualisieren lassen, jeweils mit dem Reiter "Products" oder "Casting Cell". Hier wählt man jeweils als erstes den Kunden aus. Automatisch wird der erste in der Liste ausgewählt. In diesem Beispiel ist also nichts zu tun.

- "Products" zeigt den Verlauf von bestellten Produkten dar. Zusätzlich werden die nächsten paar Monate vorrausgesagt, mithilfe von neuralen Netzen und Machine Learning.
![Aufteilung der Products Sektion](https://github.com/BerndNK/FoundryReports/blob/master/Documentation/ProductSections.PNG?raw=true)
In "A", kann man auswählen welche Produkte in "B" dargestellt werden sollen. Wählt man einen Monat aus, kann man die konkreten Werte in "C" ansehen und bearbeiten. 

Graue Werte sind geschätzt, während schwarze aus dem Datenbestand kommen. Hier kann auch erkennen, wie sich die Vorhersage anpasst, sollte man einen Wert ändern.

Diese Ansicht ist grundsätzlich wohl der einfachste Weg in das Tool Daten einzupflegen.

Bei einigen Monaten ist auf dem Trend ein oder mehrere größere Punkte zu erkennen. Diese indizieren Events in dem jeweiligen Monat. Klickt man auf diesen, so werden die jeweiligen Events in "D" gelistet.
- "Casting Cell" zeigt den Verlauf der Benutzung von Gußzellen. Der Graph funktioniert analog zu "Products". Hier sieht man allerdings keine Events, sondern es wird erläutert wie die Benutzung pro Monat berechnet wird.
![Casting Cell Sektion](https://github.com/BerndNK/FoundryReports/blob/master/Documentation/CastingCellSections.PNG?raw=true)

# Code Struktur
Die Applikation ist in 3 Projekte aufgeteilt.
- FoundryReports - NetCore 3.0 WPF Applikation
- FoundryReports.Core - Backend Business Logik
- FoundryReports.Core.Test - Tests für Business Logik

Die Struktur ist so gewählt, um eine potenzielle Migration in eine Cloud Umgebung zu ermöglichen. Der gesamte Datenaustausch passiert über das Interface [IDataSource](/FoundryReports.Core/Source/IDataSource.cs)

Jegliches Laden und Persistieren von Daten ist asynchron behandelt und führt daher nicht zu einem Einfrieren der GUI. Das heißt hinter IDataSource könnte man sehr einfach einen REST Client oder Ähnliches hängen, die für Anfragen etwas länger brauchen und die Applikation könnte damit umgehen.

## FoundryReports.Core + Test
Die 4 großen Namespaces in FoundryReports.Core sind:
- Products - Modelklassen für den Datenbestand
- Reports - Modelklassen die auf den Produktdaten aufbauen. 
- Source - Klassen die mit Laden und Persistieren von Daten zu tun haben
- Utils - Hilfsklassen für typische Szenarien

"FoundryReports.Test" enthält die selbe Struktur von Namespaces, für die jeweiligen Tests.

## FoundryReports
Da es sich um eine WPF Anwendung handelt und das typische MVVM Prinzip angewendet wurde, ergeben sich folgende Namespaces:
- Converter - Converter für etwaige Szenarien. Diese werden typischerweise statisch initialisiert. Siehe zum Beispiel [BoolToVisibility](/FoundryReports/Converter/BoolToVisibility.cs)
- Themes - Enthält Zuordnung von ViewModel zu Model
- Utils - Hilfsklassen für typische Szenarien, die nur GUI relevant sind (im Kontrast zu "FoundryReports.Utils")
- View - View Klassen für das GUI
- ViewModel - ViewModel Klassen für die jeweiligen Views

Jeweils "View" und "ViewModel" sind nochmals unterteilt in:
- CastingCell - Alle Klassen relevant für den "Casting Cell" Reiter
- DataManage - Alle Klassen relevant für den "Data" Reiter
- Graph - Alle Klassen relevant um einen Graph zu zeichnen
- Products - Alle Klassen relevant für den "Products" Reiter
- Utils - Spezielle wiederverwendbare Controls

Die Hauptansicht heißt [MainView](/FoundryReports/View/MainView.xaml) und das zugehörige ViewModel [MainViewModel](/FoundryReports/ViewModel/MainViewModel.cs), welches den Einstiegspunkt und den Startpunkt für alle Initialisierungen bildet.


## Trend Vorhersage mit Machine Learning
Zum lernen des neuralen Netzes wurde Google Colab verwendet. Das Dokument mit allen relevanten Python Skripten und Infos ist zu finden unter:
https://colab.research.google.com/drive/16uX_hq3U9WDji8nNY1bzFyh_jhE5AM1W

Zum Ausführen muss das Notizbuch kopiert werden. Die notwendigen Daten können in der FoundryReports Applikation unter "Data" -> "Customer" -> "Export" exportiert werden. Das Skript erwartet den Namen "graphData.csv". Alternativ kann auch die fertige csv [GraphData](/ExampleData/graphData.csv) verwendet werden.

Weitere Infos sind innerhalb des Notizbuches zu finden. (Englisch)

Ausgeführt wird das Netzwerk in [MlProductTrendPredictor.cs](/FoundryReports.Core/Source/Prediction/MlProductTrendPredictor.cs). Mit entsprechenden Tests in [MlProductTrendPredictorTests.cs](/FoundryReports.Core.Test/Source/Prediction/MlProductTrendPredictorTests.cs).

Das Trainieren des Netzwerks stellt sich als etwas problematisch dar, mit den wenigen Daten die zu Verfügung gestellt worden sind. Die besten Ergebnisse, wurde dadurch erzielt alle Graphen (inklusive Hochrechnungen) aneinandergereiht als einen langen Graph zu betrachten.

Das funktioniert für ein paar Monate relativ gut, führt aber dazu dass das Netzwerk denkt, jeder Graph würde auf eine normalisierte Steigung von 0.6 konvergieren. Sicherlich würde das Netzwerk sehr viel besser funktionieren, würden mehrere Jahre an Verlauf zu Verfügung stehen.

# Kompilieren
Vorraussetzung: 
- Visual Studio 2019 mit NetCore 3.0 SDK installiert ![NetCore 3 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)
- 64-Bit Rechner

"FoundryReports.sln" mit VisualStudio öffnen und über "Debug->Start" starten. In der Standardeinstellung entspricht das der F5 Taste. Visual Studio sollte automatisch NuGet Packete wiederherstellen und richtig das Projekt "FoundryReports" bauen. Wichtig ist auf x64 zu bauen (auch Tests), da die benutzten TensorFlow DLLs (via ML.Net) nur 64-Bit unterstützen.
