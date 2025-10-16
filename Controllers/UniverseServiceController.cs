using Microsoft.AspNetCore.Mvc;

namespace ClipBoardHelper.Controllers
{
    public record FunctionInfo(string functionName, string Description, string FunctionSiпnature,string callExample);
    public record  AvailFunction(List<FunctionInfo> FunctionAliases);
    public record AVAIL_FUNCTIONS_Response(List<AvailFunction> FunctionList);

    [ApiController]
    [Route("api/[controller]")]
    public class UniverseServiceController : ControllerBase
    {
        public UniverseServiceController() { }
        [HttpGet(nameof(UniverseServiceController.GET_AVAIL_FUNCTIONS))]
        [ProducesResponseType(typeof(AVAIL_FUNCTIONS_Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> GET_AVAIL_FUNCTIONS() 
        {
            var response = new AVAIL_FUNCTIONS_Response(
                [new AvailFunction(
                    [
                    new FunctionInfo("@select"," аналог sql select", "@select:=@select( ListOfCollumns) ListOfCollumns:= ColumnName Separator ListOfCollumns ", "@select(*)"),
                    new FunctionInfo("@выбрать"," аналог sql select", "@select:=@Выбрать( ListOfCollumns) ListOfCollumns:= ColumnName Separator ListOfCollumns ", "@выбрать(*)"),
                    ]),
                    new AvailFunction(
                    [
                    new FunctionInfo("@where"," аналог sql where clause ", "@where:=@where(.... ", "@select(*)@Where(SomePredicate(colmn))"),
                    
                    ]),
                    new AvailFunction(
                    [
                    new FunctionInfo("@prompt"," диалог выбора из некого набора ", "@prompt(%1,%2,%3,%4,%5,%6,%7,%8,%9,%10)", "@prompt(...)"),
                    new FunctionInfo("@Ввод"," диалог выбора из некого набора ", "@Ввод(%1,%2,%3,%4,%5,%6,%7,%8,%9,%10)", "@Ввод(...)"),
                    ]),
                    //@aggregate_aware
                    new AvailFunction(
                    [
                    new FunctionInfo("@aggregate_aware","нипонятна!", "@aggregate_aware(....)", "@aggregate_aware(...)"),
                    ]),
                ]
                );
            return Ok(response);
        }

    }
}
