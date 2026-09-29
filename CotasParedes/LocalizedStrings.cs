using Autodesk.Revit.ApplicationServices;

namespace BimBlaster.CotasParedes;

public enum AppLanguage { Portuguese, English, Spanish }

public static class LanguageResolver
{
    public static AppLanguage Resolve(LanguageType revitLanguage) => revitLanguage switch
    {
        LanguageType.Brazilian_Portuguese => AppLanguage.Portuguese,
        LanguageType.Spanish => AppLanguage.Spanish,
        LanguageType.English_USA => AppLanguage.English,
        _ => AppLanguage.Portuguese
    };
}

public static class LocalizedStrings
{
    private static readonly Dictionary<AppLanguage, Dictionary<string, string>> Strings = new()
    {
        [AppLanguage.Portuguese] = new()
        {
            ["ButtonText"] = "Cotar Paredes",
            ["ButtonTooltip"] = "Cota todas as paredes selecionadas, com opção de ignorar paredes por parâmetro e valor (ex.: REV de revestimentos).",
            ["WindowTitle"] = "Cotar Paredes",
            ["Title"] = "Cotar Paredes",
            ["DimStyleLabel"] = "Estilo de cota",
            ["FilterSection"] = "Ignorar paredes na seleção",
            ["ParamNameLabel"] = "Nome do parâmetro de TIPO (vazio = não filtrar)",
            ["ParamValueLabel"] = "Valor a ignorar",
            ["Cancel"] = "Cancelar",
            ["Run"] = "Selecionar e executar",
            ["PickPrompt"] = "Selecione as paredes abrindo uma janela na tela",
            ["TransactionName"] = "Cotar Paredes",
            ["NoStyle"] = "Nenhum estilo de cota linear encontrado no projeto.",
            ["NoWalls"] = "Nenhuma parede válida selecionada após a filtragem.",
            ["NotEnoughRefs"] = "Não foram encontradas referências (faces) suficientes para criar a cota.",
            ["NoLocation"] = "Não foi possível determinar a direção das paredes selecionadas.",
            ["Success"] = "Cota criada para {0} parede(s) selecionada(s)!",
            ["ApiError"] = "Erro da API ao gerar a cota: {0}",
        },
        [AppLanguage.English] = new()
        {
            ["ButtonText"] = "Dimension Walls",
            ["ButtonTooltip"] = "Dimensions all selected walls, with the option to ignore walls by parameter and value (e.g. REV for finishes).",
            ["WindowTitle"] = "Dimension Walls",
            ["Title"] = "Dimension Walls",
            ["DimStyleLabel"] = "Dimension style",
            ["FilterSection"] = "Ignore walls in the selection",
            ["ParamNameLabel"] = "TYPE parameter name (empty = no filter)",
            ["ParamValueLabel"] = "Value to ignore",
            ["Cancel"] = "Cancel",
            ["Run"] = "Select and run",
            ["PickPrompt"] = "Select the walls by drawing a window on screen",
            ["TransactionName"] = "Dimension Walls",
            ["NoStyle"] = "No linear dimension style found in the project.",
            ["NoWalls"] = "No valid walls selected after filtering.",
            ["NotEnoughRefs"] = "Not enough references (faces) found to create the dimension.",
            ["NoLocation"] = "Could not determine the direction of the selected walls.",
            ["Success"] = "Dimension created for {0} selected wall(s)!",
            ["ApiError"] = "API error while creating the dimension: {0}",
        },
        [AppLanguage.Spanish] = new()
        {
            ["ButtonText"] = "Acotar Muros",
            ["ButtonTooltip"] = "Acota todos los muros seleccionados, con opción de ignorar muros por parámetro y valor (ej.: REV de revestimientos).",
            ["WindowTitle"] = "Acotar Muros",
            ["Title"] = "Acotar Muros",
            ["DimStyleLabel"] = "Estilo de cota",
            ["FilterSection"] = "Ignorar muros en la selección",
            ["ParamNameLabel"] = "Nombre del parámetro de TIPO (vacío = sin filtro)",
            ["ParamValueLabel"] = "Valor a ignorar",
            ["Cancel"] = "Cancelar",
            ["Run"] = "Seleccionar y ejecutar",
            ["PickPrompt"] = "Seleccione los muros dibujando una ventana en pantalla",
            ["TransactionName"] = "Acotar Muros",
            ["NoStyle"] = "No se encontró ningún estilo de cota lineal en el proyecto.",
            ["NoWalls"] = "Ningún muro válido seleccionado tras el filtrado.",
            ["NotEnoughRefs"] = "No se encontraron suficientes referencias (caras) para crear la cota.",
            ["NoLocation"] = "No se pudo determinar la dirección de los muros seleccionados.",
            ["Success"] = "¡Cota creada para {0} muro(s) seleccionado(s)!",
            ["ApiError"] = "Error de la API al generar la cota: {0}",
        },
    };

    public static string Get(LanguageType revitLanguage, string key) =>
        Get(LanguageResolver.Resolve(revitLanguage), key);

    public static string Get(AppLanguage language, string key) =>
        Strings[language].GetValueOrDefault(key, Strings[AppLanguage.Portuguese][key]);
}
