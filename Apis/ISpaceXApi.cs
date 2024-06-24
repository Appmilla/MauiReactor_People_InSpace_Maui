using System.Threading.Tasks;
using Refit;

namespace MauiReactorPeopleInSpace.Apis;

public interface ISpaceXApi
{        
    [Get("/crew")]
    Task<string> GetAllCrew();
}