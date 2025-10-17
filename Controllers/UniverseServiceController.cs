using Microsoft.AspNetCore.Mvc;

namespace ClipBoardHelper.Controllers
{
    public record Formula_Function_Argument(string Descr);

    public record ParametersSignatures(
        List<Formula_Function_Argument> args,
        Formula_Function_Argument result);

    public record Formula_One_Function(
        List<string> names,
        bool isOperator,
        string descr,
        bool visible,
        ParametersSignatures types);

    public record GET_AVAIL_FUNCTIONS_RESULT(
        List<Formula_One_Function> functions,
        int result);

    [ApiController]
    [Route("api/[controller]")]
    public class UniverseServiceController : ControllerBase
    {
        public UniverseServiceController()
        {
        }

        [HttpGet(nameof(GET_AVAIL_FUNCTIONS))]
        [ProducesResponseType(typeof(GET_AVAIL_FUNCTIONS_RESULT), StatusCodes.Status200OK)]
        public async Task<IActionResult> GET_AVAIL_FUNCTIONS()
        {
            var response = new GET_AVAIL_FUNCTIONS_RESULT(
                functions: new List<Formula_One_Function>
                {
                    new Formula_One_Function(
                        names: new List<string> { "@select", "@выбратьX" },
                        isOperator: true,
                        descr: "аналог sql select",
                        visible: true,
                        types: new ParametersSignatures(
                            args: new List<Formula_Function_Argument>
                            {
                                new Formula_Function_Argument("Column Name"),
                                new Formula_Function_Argument("*")
                            },
                            result: new Formula_Function_Argument("void"))),

                    new Formula_One_Function(
                        names: new List<string> { "@where" },
                        isOperator: false,
                        descr: "аналог sql where",
                        visible: true,
                        types: new ParametersSignatures(
                            args: new List<Formula_Function_Argument>
                            {
                                new Formula_Function_Argument("Предикат отбора")
                            },
                            result: new Formula_Function_Argument("void"))),

                    new Formula_One_Function(
                        names: new List<string> { "@prompt", "@Ввод" },
                        isOperator: false,
                        descr: "диалог выбора из некого набора",
                        visible: true,
                        types: new ParametersSignatures(
                            args: new List<Formula_Function_Argument>
                            {
                                new Formula_Function_Argument("PromptP1"),
                                new Formula_Function_Argument("PromptP2"),
                                new Formula_Function_Argument("PromptP2"),
                                new Formula_Function_Argument("PromptP3"),
                                new Formula_Function_Argument("PromptP4")
                            },
                            result: new Formula_Function_Argument("выбранный элемент, тип определяется типом колонки источника"))),

                    new Formula_One_Function(
                        names: new List<string> { "@aggregate_aware" },
                        isOperator: false,
                        descr: "чтото непонятное",
                        visible: false,
                        types: new ParametersSignatures(
                            args: new List<Formula_Function_Argument>
                            {
                                new Formula_Function_Argument("aggregate_aware argument")
                            },
                            result: new Formula_Function_Argument("aggregate_aware result")))
                },
                result: 0);

            return Ok(response);
        }
    }
}