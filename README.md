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
|               |   +---AddProductPage
|               |   \---SearchPage
|               \---lib
|   \---doc
|       +---docgen
|   \---test
\---test
    \---UnitTest
```

- El directorio [./data/]("./data") contiene informaci�n importante para generar los datos para la base de datos
- El directorio [./design/]("./design/") contiene los archivos de dise�o
- El directorio [./design/database/]("./design/database/") contiene los diagramas de la base de datos
- El directorio [./design/mockups/]("./design/mockups/") contiene los mockups de las p�ginas implemetadas durante el sprint
- El directorio [./design/uml/]("./design/uml/") contiene los diagramas del modelo del sistema
- El directorio [./source/]("./source/") contiene el c�digo e archivos importantes
- El directorio [./source/LoCoMPro/LoCoMPro/]("./source/LoCoMPro/LoCoMPro/") contiene los archivos con el c�digo del sistema
- El directorio [./source/doc/]("./source/doc/") contiene los para realizar la documentaci�n
- El directorio [./source/test/]("./source/test/") contiene los test unitarios del proyecto
