# TrackingMarquesString

Aplicació desarrollada en MAUI i .NET 7.0.

L'aplicació està pensada per ser emprada en les sortides que es realitzen en M4 - Llenguatge de Marques i Sistemes de Gestió de la Informació als cicles superiors DAM-DAW-ASIX

Features:

Marcar punts de ruta
Marcars punts d'interés amb un nom
Guardar la ruta en el següent format:

<root>
    <puntsRuta>
        <ruta>
            <latitud>41.9763584</latitud>
            <longitud>2.8140623</longitud>
            <elevacio>123.9000015258789</elevacio>
            <dataHora>2023-02-28T14:13:17Z</dataHora>
        </ruta>
        <ruta>
            <latitud>41.976497315635946</latitud>
            <longitud>2.8144541386520507</longitud>
            <elevacio>120.98018694178198</elevacio>
            <dataHora>2023-02-28T14:13:22Z</dataHora>
        </ruta>
        .
        .
        .
    </puntsRuta>
    <puntsInteres>
        <punt>
            <nom>Estació d'autobusos</nom>
            <latitud>41.9763739809965</latitud>
            <longitud>2.814621075231793</longitud>
            <elevacio>122.83910570206041</elevacio>
            <dataHora>2023-02-28T14:13:51Z</dataHora>
        </punt>
        <punt>
            <nom>parking</nom>
            <latitud>41.975899058813845</latitud>
            <longitud>2.8160115980172</longitud>
            <elevacio>124.0365448226414</elevacio>
            <dataHora>2023-02-28T14:15:40Z</dataHora>
        </punt>
        .
        .
        .
    </puntsInteres>
</root>
