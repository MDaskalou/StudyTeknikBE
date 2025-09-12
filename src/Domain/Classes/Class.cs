namespace Domain.Classes
{
    // TODO: Aggregate Root för skolklass.
    //Aggregate menas att en klass är en rot i en trädstruktur av objekt.
    //Many-to-one relation med User (en klass har många elever, en elev tillhör en klass).
    //Todo: // - Unik kombination (SchoolName, Year, ClassName) i DB.
    //Todo: Äger sina Enrollment-relationer i domänmodell.
    //Enrollment menas relationen mellan User och Class.
    // TODO: validera year (rimlig range), icke tomma strängar.

    //Klassen som elever tillhör (t.ex. “9B på Bräckeskolan”).
    public class Class
    {
        
    }
}