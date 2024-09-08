using Microsoft.AspNetCore.Mvc;

namespace Tracking.Requests;

internal record PostEventRequest([FromRoute] long AccountId, [FromBody] PostEventBody Body);
internal record PostEventBody(string Data);