using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ureeka_backend.Services;

namespace ureeka_backend.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TestDataController : ControllerBase
{
    private readonly IRedisCacheService _redisCacheService;

    public TestDataController(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    
    [HttpGet]
    public string GetData()
    {
        var cars = _redisCacheService.GetData<string>("val");
        if (cars is not null)
        {
            return "Cached : " + cars;
        }

        var strVal = "Honda Civic";
        _redisCacheService.SetData<string>("val", strVal);
        return strVal;
    }
}