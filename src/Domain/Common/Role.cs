namespace Domain.Common
{
    
    //fasta listor som används i hela domänen
    //Todo: Synkar med claims från idp i WebApi/infrastrucure
    public enum Role
    {
        Student,
        Teacher,
        Admin,
        Mentor
        
    }
}