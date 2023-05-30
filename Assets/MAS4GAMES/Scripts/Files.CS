using System;
using System.IO;

Class FileManager {
    public string FileName; //Nombre del Archivo
    public string FilePath; //Dirección completa del archivo (dónde se guardará)
    private string FilePathIncomplete = @"\PRY20231003-MAS4GAMES\Assets\MAS4GAMES\Logs"; //Sólo la ruta (no contiene el nombre del archivo)
    public enum Actions {ATTACK, BLOCK, MOVE,  DODGE}; //Acciones para el contenido

    FileManager() {
        DateTime Datetime =  = DateTime.Now;; //Fecha actual
        string name = "Partida " + Datetime.ToString() + ".txt"; //Crear el nombre del archivo 

        Filename = name; //Nombre completo
        FilePath = Path.Combine(FilePath, FileName); //La ruta final del archivo
    }

    //Función para escribir en el archivo
    public void WriteFile(string content) {
        Directory = this->FilePath; //Obtener la dirección del archivo
        StreamWriter write = = new StreamWriter(Directory); //Abrir el archivo
        write.Write(content); //Escribir en el archivo
        escritor.Close(); //Cerrar el archivo
    }
}