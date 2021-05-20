using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

using M = Microsoft.AspNetCore.Mvc.ApiExplorer.ApiConventionNameMatchBehavior;
using T = Microsoft.AspNetCore.Mvc.ApiExplorer.ApiConventionTypeMatchBehavior;

namespace ProfanityChecker.WebApi
{
    public static class ApiConventions
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ApiConventionNameMatch(M.Exact)]
        public static void Get(
            [ApiConventionNameMatch(M.Suffix), ApiConventionTypeMatch(T.Any)] object id, params object[] other)
        {
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ApiConventionNameMatch(M.Prefix)]
        public static void GetBy(
            [ApiConventionNameMatch(M.Any), ApiConventionTypeMatch(T.Any)] object param, params object[] other)
        {
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ApiConventionNameMatch(M.Exact)]
        public static void GetAll(
            [ApiConventionNameMatch(M.Exact), ApiConventionTypeMatch(T.AssignableFrom)] int? skip,
            [ApiConventionNameMatch(M.Exact), ApiConventionTypeMatch(T.AssignableFrom)] int? take,
            params object[] other)
        {
        }
        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ApiConventionNameMatch(M.Exact)]
        public static void Create(
            [ApiConventionNameMatch(M.Any), ApiConventionTypeMatch(T.Any)] object model, params object[] other)
        {
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ApiConventionNameMatch(M.Exact)]
        public static void Update(
            [ApiConventionNameMatch(M.Suffix), ApiConventionTypeMatch(T.Any)] object id,
            [ApiConventionNameMatch(M.Any), ApiConventionTypeMatch(T.Any)] object model,
            params object[] other)
        {
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ApiConventionNameMatch(M.Exact)]
        public static void Delete(
            [ApiConventionNameMatch(M.Suffix), ApiConventionTypeMatch(T.Any)] object id,
            [ApiConventionNameMatch(M.Any), ApiConventionTypeMatch(T.Any)] object model,
            params object[] other)
        {
        }
    }
}