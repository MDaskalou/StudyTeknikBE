namespace Domain.Common
{
    //Bas entity som alla entities i domänen ärver från
    // Det kommer innehålla gemensamma properties
    // I den här kommer det vara ID, createdAt, updatedAt
    //Touch är en metod som uppdaterar updatedAt
    //Detta är för att undvika repetition av dessa properties i alla entities
    
    //Todo: Generisk class för Id, CreatedAt, UpdatedAt
    //Todo: Anropar Touch() i repositoryn när en entity uppdateras
    
    public abstract class Entity
    {
        
    }
}