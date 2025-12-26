using CoreWCF;
using SmartStudyRooms.Data.Models;
using System.Collections.Generic;

[ServiceContract(Namespace = "http://smartstudyrooms.ipca.pt/soap")]
public interface ISalaSoapService
{
    [OperationContract]
    IEnumerable<Sala> ListarSalas();

    [OperationContract]
    Sala ObterSala(int id);

    [OperationContract]
    int CriarSala(Sala sala);


}
