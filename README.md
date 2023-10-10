# Database Drafts

WebApp que permite encontrar el mejor precio en un establecimiento cercano de un producto buscado.


## Integrantes

- [Dwayne Taylor | C17827](https://github.com/Dwayne-T)
- [Alonso Le�n | B94247](https://github.com/Alr201)
- [Omar Camacho | C11476](https://github.com/OmArCaMc)
- [Julio Alejandro | C16717](https://github.com/JulioAleRodri)
- [Geancarlo Rivera | C06516](https://github.com/JGeanca)

## Informaci�n adicional
**Curso:**
- CI-0128 Proyecto Integrador de Ingenier�a de Software y Bases de Datos

**Profesores:**
- Dr. Allan Berrocal Rojas
- Dra. Alexandra Mart�nez

### LoCoMPro

En la empresa, **_Buen Software S.A._**, se han planteado desarrollar una aplicaci�n que
permita a sus usuarios gestionar el proceso de Localizaci�n y Consulta del Mejor
Producto, le han llamado **LoCoMPro**. Esta ser�a una aplicaci�n de software que
permita a las personas colaborar generando informaci�n sobre precios de productos, y
al mismo tiempo beneficiarse de esta informaci�n para sus propias compras.

## Estructura de archivos
```plaintext
+---data
+---design
|   \---sprint0
|       +---database
|       |   +---avance1
|       |   \---avance2
|       \---mockups
|           +---avance1
|           \---avance2
|   \---sprint1
|      +---mockups
|          +---avance1
|      \---uml
|          +---avance2
|      \---database
|          +---avance2
+---source
|   \---LoCoMPro
|       \---LoCoMPro
|           +---Areas
|           |   \---Identity
|           |       \---Pages
|           |           \---Account
|           |               \---Manage
|           +---Data
|           +---Models
|           +---Pages
|           |   \---Shared
|           +---Properties
|           +---Utils
|           \---wwwroot
|               +---css
|               +---img
|               +---js
|               \---lib
|   \---doc
|       +---docgen
\---test
    \---UnitTest
```

- El directorio ["./data/"](./data) contiene informaci�n importante para generar los datos para la base de datos
- El directorio ["./design/"](./design/) contiene los archivos de dise�o
- El directorio ["./design/sprint1/avance1/database/"](./design/sprint1/avance1/database/) contiene los diagramas de la base de datos
- El directorio ["./design/sprint1/avance2/mockups/"](./design/sprint1/avance2/mockups/) contiene los mockups de las p�ginas implemetadas durante el sprint
- El directorio ["./design/sprint1/avance2/uml/"](./design/sprint1/avance2/uml/) contiene los diagramas del modelo del sistema
- El directorio ["./source/"](./source/) contiene el c�digo e archivos importantes
- El directorio ["./source/LoCoMPro/LoCoMPro/"](./source/LoCoMPro/LoCoMPro/) contiene los archivos con el c�digo del sistema
- El directorio ["./source/doc/"](./source/doc/) contiene los para realizar la documentaci�n
- El directorio ["./test/"](./test/) contiene los test unitarios del proyecto

# Manual de usuario 

## LoCoMpro

LoCoMProp es una aplicaci�n que apunta a la cooperaci�n entre los usuarios para proveer informaci�n precisa sobre los precios y ubicaci�n de productos.

## Descripci�n

El software es un sistema que integra un motor de b�squeda para permitir a los usuarios buscar y descubrir productos de su interes, adem�s de proveer la capacidad de calificar rese�as subidas por otros usuarios sobre un producto que se encuentra en un establecimiento espec�fico.

## Informaci�n de acceso

El sistema web LoCoMPro permite a los usuarios iniciar sesi�n y registarse en la aplicaci�n por medio de su correo el�ctronico y una contrase�a. Los usuarios que no poseen una cuenta de LoCoMPro, puede crear una al registrase por medio de su correo eletr�nico, su nombre de usuario y una contrase�a. Estas credenciales deben seguir ciertas reglas para para ser consideradas v�lidas por el sistema, como no sobrepasar cierto l�mite de caract�res, poseer letras en may�scula y min�scula o contar con n�meros y caract�res especiales, adem�s de que no pueden existir dos correos y nombres de usuarios iguales en el sistema.

Lo usuario que poseen una cuenta de LoCoMPro pueden iniciar sesi�n utilizando el mismo correo y contrase�a que ingresaron al momento de registrarse. Una vez se inicio sesi�n, el usuario tiene la opci�n de cerrar sesi�n en caso de que ya no desee ser identificado por su cuenta.

Todos lo usuario de la aplicaci�n pueden utilizar el motor de b�squeda para realizar b�squedas de productos y tambi�n pueden pueden ingresar a la p�gina que muestra los registros de un producto en una tienda en espec�fico. Un usuario que no haya iniciado sesi�n no puede agregar nuevos registros en el sistema ni tampoco puede interactuar con los registros subidos por otros usuarios, para ello debe iniciar sesi�n, o en el caso de no poseer una cuenta, registrase.

Para registrar un nuevo usuario se debe tener en cuenta las siguientes restricciones:

**Correo**
- Debe contar con el formato estandar de un correo electr�nico
- Contar con el s�mbolo "@" y un dominio v�lido, por ejemplo "@gmail.com" o "ucr.ac.cr"
- Contar con TLD
- No se puede utilizar un correo que haya sido ingresado para otra cuenta

**Usuario**
- No sobrepasar el l�mite de caract�res
- Puede contar con may�sculas, min�sculas, n�meros, espacios y cierta lista de caract�res adicionales
- No puede componerse de solamente espacios en blanco
- No se puede utilizar un nombre de usuario que haya sido ingresado para otra cuenta

**Contrase�a**
- Debe de contar con al menos 6 caracteres
- Debe contar con al menos una letra may�scula
- Debe contar con al menos una letra min�scula
- Contar con al menos un n�mero
- Contar con al menos un signo de puntuaci�n
- No puede estar compuesta completamente de espacios en blanco

## Uso de las funcionalidades de la aplicaci�n

La aplicaci�n LoCoMPro busca brindar un entorno f�cil de utilizar, intuitivo y confiable para realizar b�squeda de productos, adem�s basarse en los aportes realizados por los usuarios del sistema para brindar informaci�n ver�dica. Sumado a lo anterior, posee funcionalidades que facilitan y extienden las ya mencionadas, con el proposito de brindar mayor control y poder a los usuario al momento e utilizar el sistema.

### Registrarse

El sistema permite a los usuario que no cuentan con una cuenta de LoCoMPro registrarse ingresando un correo electr�nico, un nombre de usuario y una contrase�a.

![Imagen de registro de usuario](./img/Registrarse.png)

### Iniciar sesi�n

Permite a los usuario que ya poseen una cuenta ingresar y ser identificados por el sistema al ingresar el correo y contrase�a utilizada para crear la cuenta.

![Imagen de iniciar sesi�n](./img/IniciarSesion.png)

### Buscar un producto

El motor de b�squeda de LoCoMPro permite a los usuarios buscar productos que fueron ingresados por otros usuario por medio de una barra de b�squeda. El puede buscar por medio del nombre, la marca o el modelo del producto y se le mostrar�n los resultados de dicha b�squeda que coincidan con los datos que se encuentran en el sistema.

![Imagen de b�squeda](./img/Busqueda.png)

### Visualizar un producto

La aplicaci�n permite al usuario ver los datos de un producto espec�fico que se encuentra en un establecimiento en espec�fico, adem�s de observar los registros ingresados por otros usuarios.

![Imagen de visualizaci�n de producto](./img/VisualizarProducto.png)

### Agregar registro

Se le permite a los usuario que se encuentre registrados agregar nuevos registros de un producto espec�fico en un establecimiento en espec�fico, a�adiendo informaci�n importante que pueda ser visualizada por otros usuarios.

![Imagen de agregar registro](./img/AgregarRegistro.png)

### Ordenar resultados

Al realizar una b�squeda o visualizar los registros de un producto, es posible elegir el orden de los registros que se muestran seg�n diversos atributos.

![Image ordenar registros](./img/Ordenar.png)

### Filtrar resultados

Al realizar una b�squeda, es posible filtrar los resultados obtenidos seg�n diversos stributos para limitar los resultados obtenidos.

![Imagen filtar registros](./img/Filtrar.png)

### Elegir ubicaci�n

El sistema LoCoMPro permite a los usuario elegir una ubicaci�n que va a ser tomada como referencia al momento de realizar b�squedas.

![Image elegir ubicaci�n](./img/ElegirUbicacion.png)

### Autocompletado

Al momento de agregar un nuevo registro de un producto, el sistema puede recomendar al usuario establecimiento y productos ya existentes en los datos de la aplicaci�n. Sumado a lo anterior, dado que se use el autocompletado, el sistema llenar� la informaci�n de la categor�a, marca y modelo del producto de manera autom�tica.

![Imagen autocompletado](./img/Autocompletado.png)

# Manual T�cnico
## Prerequisitos

Para la correcta ejecuci�n del programa se requiere:
- Microsoft Visual Studio 2022 versi�n Community.
- Dependencias de ASP.NET instaladas en Visual Studio.
- Aplicaci�n para conectarse al VPN de la ECCI, en este caso se est� utilizando Pritunl.

## Instalaci�n

Para comenzar con la instalaci�n y los paquetes, primero descargue el instalador de Visual Studio 2022 versi�n Community diriji�ndose al siguiente link: [Descargar Visual Studio 2022]
(https://visualstudio.microsoft.com/es/downloads/). Saldr�n varias opciones de descarga, se recomienda elegir la versi�n "Comunidad".

Una vez descargado el instalador, ejec�telo y en el primer di�logo presione "Continuar", con esto Visual Studio comenzar� a descargarse e instalarse autom�ticamente en su computadora como se observa en la siguiente imagen:

![Imagen de instalaci�n VS1](./img/vs_installation/InstalacionVS_1.PNG)


Una vez completada la descargada e instalaci�n se deben instalar las dependencias ASP.NET, para esto seleccione la versi�n de Visual Studio que recien instal� y haga clic en el bot�n "modify" como se muestra en la siguiente imagen:

![Imagen de instalaci�n VS2](./img/vs_installation/InstalacionVS_2.PNG)

Luego en la ventana emergente seleccione las casillas de ASP.NET and web development y Azure development y seleccione "Install" y espere a que se instalen:

![Imagen de instalaci�n VS3](./img/vs_installation/InstalacionVS_3.PNG)

## Preparaci�n de la base de datos
La aplicaci�n requiere de una base de datos para almacenar toda su informaci�n, para esto se pueden seguir dos m�todos: utilizar una base de datos local instalada en una computadora propia o utilizar la base de datos proporcionada por la ECCI.

### Utilizar una base de datos local
Para utilizar una base de datos local se debe instalar SQL Server Expresss en el siguiente link: [Descargar SQL Server Express] (https://www.microsoft.com/en-us/sql-server/sql-server-downloads). Una vez instalado tambi�n es recomendable instalar SQL Server  Management Studio, que le permite administrar el servidor de bases de datos de forma visual. El enlace para descargarlo es el siguiente: [SQL Server Management Studio] (https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16#download-ssms).

Para configurar la aplicaci�n para que utilice la base de datos local es necesario editar el archivo `appsettings.json` para que el ConectionStrings contenga algo parecido a lo siguiente siguiente: 
```
"ConnectionStrings": {
  "LoCoMProContext": "Server=(localdb)\\mssqllocaldb;Database=LoCoMProContext;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Utilizar la base de datos de la ECCI
Para utilizar la base de datos proporcionada por la ECCI, primero hay que conectarse a la red de la escuela mediante un VPN Client. En este caso en espec�fico se va a utilizar Pritunl, el cual se puede descargar mediante el siguiente link: [Pritunl Client] (https://client.pritunl.com/#install). A continuaci�n siga el siguiente enlace para obtener una descripci�n detallada de c�mo conectarse a la VPN de la ECCI: [VPN ECCI] (https://wiki.ecci.ucr.ac.cr/estudiantes/vpn).

Para configurar la aplicaci�n para trabaje con la base de datos proporcionada por la escuela modifique el campo de ConectionStrings en `appsettings.json` para que contenga lo siguiente:
```
"ConnectionStrings": {
  "LoCoMProContext": "Server=172.16.202.209;Database=Equipo1;User Id=Equipo1Admin;Password=ZwMcPuQjo37641.;Trusted_Connection=False;TrustServerCertificate=True;"
}
```

## Ejecuci�n de la Aplicaci�n
Una vez teniendo todos los prerequisitos de instalaci�n, ejecute Visual Studio, continuando con los pasos anteriores puede hacerlo presionando el bot�n "Launch":
![Imagen de instalaci�n VS4](./img/vs_installation/InstalacionVS_4.PNG)

Una vez abierto Visual Studio, vamos a descargar el proyecto LoCoMPro desde el repositorio de GitHub, para esto, en la primera ventana que se muestra al abrir Visual seleccione la opci�n "Clone a repository" como se observa en la siguiente imagen:

![Imagen de instalaci�n VS5](./img/vs_installation/InstalacionVS_5.PNG)

Seleccione el path o ubicaci�n en su computadora donde desea que se guarde el proyecto, luego en la caja "Repository location" copie y pegue el siguiente enlace que corresponde al repositorio del proyecto: https://github.com/Alr201/ci0128_23b_database_drafts.git, por �ltimo presione el bot�n "Clone", puede ver este proceso en la siguiente imagen:

![Imagen de instalaci�n VS6](./img/vs_installation/InstalacionVS_6.PNG)

Con esto, ya tendr� el proyecto en la ubicaci�n seleccionada en el paso anterior, ahora para abrirlo en Visual Studio, vaya al "Solution Explorer" y en la ubicaci�n: `source/LoCoMPro` y de doble click al archivo llamado `LoCoMPro.sln`. Este proceso se muestra en la siguiente imagen:

![Imagen de instalaci�n VS7](./img/vs_installation/InstalacionVS_7.PNG)

*Nota*: En caso de que no visualice el "Solution Explorer" de Visual Studio, puede abrirlo en la barra de opciones, en la secci�n de View y presionando "Solution Explorer" o  presionando la combinaci�n de teclas Ctrl+Alt+l en su teclado.


Como �ltimo paso simplemente ejecute la aplicaci�n dando click al bot�n de la flecha verde como se muestra en la siguiente imagen: 

![Imagen de instalaci�n VS8](./img/vs_installation/InstalacionVS_8.PNG)

Si aplic� todos estos pasos, entonces se abrir� en su navegador la aplicaci�n y ya podr� comenzar a interactuar con ella:

![Imagen de instalaci�n VS9](./img/vs_installation/InstalacionVS_9.PNG)

*Nota*: Puede elegir el navegador de su preferencia en el que quiere que se abra la aplicaci�n dando click en el desplegable que se encuentra ubicado a la derecha de la flecha verde con la que ejecuta el programa.


## Ejecutar Tests
Para ejecutar los tests de la aplicaci�n abra el proyecto en Visual Studio y en la barra de opciones, en la secci�n Test seleccione Test Explorer como se muestra en la siguiente imagen:

![Imagen Tests1](./img/tests/EjecutarTests_1.png)

El paso anterior abrir� una ventana emergente donde puede ver el detalle de los tests disponibles, para ejecutarlos simplemente presione el bot�n de la flecha verde como se muestra en la siguiente imagen:

![Imagen Tests2](./img/tests/EjecutarTests_2.PNG)

Una vez ejecutado los tests, podr� ver y explorar los resultados de cada uno, la siguiente imagen muestra el caso en el que todos los tests disponibles pasaron satisfactoriamente:

![Imagen Tests3](./img/tests/EjecutarTests_3.PNG)

## Contact

- [Dwayne Taylor | C17827](https://github.com/Dwayne-T)
- [Alonso Le�n | B94247](https://github.com/Alr201)
- [Omar Camacho | C11476](https://github.com/OmArCaMc)
- [Julio Alejandro | C16717](https://github.com/JulioAleRodri)
- [Geancarlo Rivera | C06516](https://github.com/JGeanca)



