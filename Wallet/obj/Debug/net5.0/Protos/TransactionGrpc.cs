// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/transaction.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GrpcServer {
  public static partial class Customer
  {
    static readonly string __ServiceName = "Customer";


    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GrpcServer.TransactionReflection.Descriptor.Services[0]; }
    }

    /// <summary>Client for Customer</summary>
    public partial class CustomerClient : grpc::ClientBase<CustomerClient>
    {
      /// <summary>Creates a new client for Customer</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public CustomerClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Customer that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public CustomerClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected CustomerClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected CustomerClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override CustomerClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new CustomerClient(configuration);
      }
    }

  }
}
#endregion