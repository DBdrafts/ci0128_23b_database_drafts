# Database Drafts

WebApp que permite encontrar el mejor precio en un establecimiento cercano de un producto buscado.


## Integrantes

- [Dwayne Taylor | C17827](https://github.com/Dwayne-T)
- [Alonso León | B94247](https://github.com/Alr201)
- [Omar Camacho | C11476](https://github.com/OmArCaMc)
- [Julio Alejandro | C16717](https://github.com/JulioAleRodri)
- [Geancarlo Rivera | C06516](https://github.com/JGeanca)

## Información adicional
**Curso:**
- CI-0128 Proyecto Integrador de Ingeniería de Software y Bases de Datos

**Profesores:**
- Dr. Allan Berrocal Rojas
- Dra. Alexandra Martínez

### LoCoMPro

En la empresa, **_Buen Software S.A._**, se han planteado desarrollar una aplicación que
permita a sus usuarios gestionar el proceso de Localización y Consulta del Mejor
Producto, le han llamado **LoCoMPro**. Esta sería una aplicación de software que
permita a las personas colaborar generando información sobre precios de productos, y
al mismo tiempo beneficiarse de esta información para sus propias compras.

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

- El directorio [./data/]("./data") contiene información importante para generar los datos para la base de datos
- El directorio [./design/]("./design/") contiene los archivos de diseño
- El directorio [./design/database/]("./design/database/") contiene los diagramas de la base de datos
- El directorio [./design/mockups/]("./design/mockups/") contiene los mockups de las páginas implemetadas durante el sprint
- El directorio [./design/uml/]("./design/uml/") contiene los diagramas del modelo del sistema
- El directorio [./source/]("./source/") contiene el código e archivos importantes
- El directorio [./source/LoCoMPro/LoCoMPro/]("./source/LoCoMPro/LoCoMPro/") contiene los archivos con el código del sistema
- El directorio [./source/doc/]("./source/doc/") contiene los para realizar la documentación
- El directorio [./source/test/]("./source/test/") contiene los test unitarios del proyecto
