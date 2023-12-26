

using Grpc.Core;
using Grpc.Net.Client;
using Servicemanegepolicyclient.Protos;

var channel = GrpcChannel.ForAddress("http://localhost:5092");

var policyClient=new Servicemanegepolicyclient.Protos.PolicyServiceGRPC.PolicyServiceGRPCClient(channel);

//using var serverStream=policyClient.GetAllPolicies(new Google.Protobuf.WellKnownTypes.Empty());
//await foreach(var p in serverStream.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine($"{p.PolicyIdx} / {p.Reference} / {p.PolicyHolder}");
//}

var prequest = new PolicyRequest()
{
    PolicyIdx = 2
};

var getSinglePolicyCall = policyClient.GetSinglePolicyAsync(prequest);
var p = await getSinglePolicyCall.ResponseAsync;

Console.WriteLine($"{p.PolicyIdx} / {p.Reference} / {p.PolicyHolder}");

channel.Dispose();
await channel.ShutdownAsync();
