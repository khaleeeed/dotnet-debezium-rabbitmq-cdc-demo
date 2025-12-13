---
theme: seriph
class: 'text-center'
highlighter: shiki
lineNumbers: false
info: |
  ## Modern Event-Driven Data Architecture
  Exploring Domain Events, CDC Debezium, and Advanced Patterns
drawings:
  persist: false
css: unocss
layout: cover
---

<style>
/* Global Aesthetics */
:root {
  --tasheer-blue: #1A468B;
  --tasheer-gray: #64748b;
}

/* H1 as the Top Header Banner */
h1 {
  background-color: var(--tasheer-blue);
  color: white !important; /* Force white text on blue background */
  font-family: 'Inter', sans-serif;
  letter-spacing: -0.025em;
  
  /* Position as top banner */
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  padding: 1.5rem 3rem;
  margin: 0;
  z-index: 10;
  
  /* Typography */
  font-size: 2.25rem;
  line-height: 2.5rem;
  font-weight: 700;
}

/* Adjust content to not be hidden behind fixed H1 */
.slidev-layout:not(.cover) {
  padding-top: 6rem;
}

h2, h3, h4 {
  font-family: 'Inter', sans-serif;
  letter-spacing: -0.025em;
  color: var(--tasheer-blue);
}

/* Override cover slide headings to be white and centered (normal flow) */
.slidev-layout.cover h1 {
    position: relative;
    background-color: transparent;
    padding: 0;
    margin-bottom: 1.5rem;
    color: white !important;
}
.slidev-layout.cover h2 {
    color: white !important;
}

strong {
  color: var(--tasheer-blue);
  font-weight: 700;
}

/* Hide default Slidev UI elements (Navigation, Controls) to remove "black pointer" artifacts */
.slidev-controls,
.slidev-nav,
#slidev-handle {
  display: none !important;
}
</style>

<!-- Removed static Global Header Component -->

<div class="absolute inset-0 bg-[#1A468B] -z-10"></div>

<!-- RELOCATED DEBEZIUM IMAGE: Bottom right, small size -->
<img src="/image.png" class="absolute bottom-4 right-4 w-32 hover:grayscale-0 transition duration-300" alt="Debezium Logo" />

<div class="flex flex-col items-center justify-center h-full">
  <!-- Tasheer Logo -->
  <img src="/tasheer-logo.svg" class="h-32 mb-8 hover:scale-105 transition-all duration-500 brightness-0 invert" alt="Tasheer Logo" />
  
  <h1 class="text-6xl font-extrabold tracking-tight !leading-tight text-white mb-6">
  Modern Event-Driven Data Architecture
  </h1>
  
  <p class="text-2xl text-slate-200 font-medium max-w-2xl mx-auto leading-relaxed">
  Domain Events, CDC with Debezium<br/>for Scalable Microservices
  </p>


</div>

<!--
Welcome everyone to this presentation on Modern Event-Driven Data Architecture. Today we'll explore how Domain Events, Change Data Capture (CDC), and Debezium enable scalable and maintainable microservices architectures. We'll dive into advanced patterns including CQRS, Outbox, Saga, and Event Sourcing.
-->

---
layout: two-cols
---

# Domain Events

### Definition
Events that represent significant business occurrences within a bounded context

### Key Benefits

- **Decoupling** - Services don't need direct dependencies
- **Scalability** - Independent service scaling
- **Real-time Reactions** - Immediate downstream processing
- **Audit Trail** - Complete history of business events


::right::

<div class="mt-12 ml-4 flex flex-col items-center justify-center text-center">


<img src="/domain-events-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="Domain Events Example" />

<div class="text-xs mt-4 opacity-75">
Single event triggers multiple independent reactions
</div>

</div>

<!--
Domain Events are business-significant occurrences that happen within your system. When an order is placed, that's not just a database update—it's a business event that triggers multiple reactions. The Inventory Service needs to reserve items, Payment Service processes payment, Notification Service sends confirmations, and Analytics tracks the sale. All these services react independently without tight coupling.
-->

---
layout: default
---

# Challenges of Implementing Domain Events

<div class="grid grid-cols-2 gap-8 mt-8">
<div>

### Technical Challenges

- **Event Ordering** - Maintaining sequence across services
- **Reliable Delivery** - Guaranteeing at-least-once delivery
- **Data Consistency** - Keeping distributed state synchronized
- **Duplication** - Handling duplicate event delivery
- **Failure handling** - Failure handling & retries


</div>
<div>


<div class="mt-8">

<img src="/challenges-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="Challenges Diagram" />

</div>

</div>
</div>



<!--
While Domain Events provide many benefits, implementing them correctly is challenging. Events can arrive out of order, be duplicated, or lost entirely. Multiple services maintaining their own state can become inconsistent. When failures occur, we need strategies to compensate and recover. These challenges are exactly what CDC and Debezium help solve.
-->

---
layout: default
---

# CDC & Debezium Overview

<div class="grid grid-cols-2 gap-6">
<div>

### Change Data Capture (CDC)
Technique for tracking and capturing database changes in real-time

### Debezium Features

- **Database Support** - MySQL, PostgreSQL, MongoDB, etc.
- **Message Broker Support** - Kafka,Rabbitmq, Amazon Kinesis, etc.
- **Low Latency** - real-time capture
- **No Code Changes** - Non-intrusive


</div>
<div class="mt-4">

<img src="/cdc-overview-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="CDC Overview" />

<div class="text-xs mt-2 opacity-75">
Debezium reads transaction logs and streams changes to Kafka
</div>

</div>
</div>

<!--
Change Data Capture is the foundation that solves many of our challenges. Instead of your application code publishing events, Debezium monitors the database transaction log—the same log used for replication and backups. This means every committed transaction is captured reliably and streamed to Kafka. It's non-intrusive: no changes to your application code, no performance impact, and guaranteed consistency.
-->

---
layout: default
---

# How Debezium Supports Domain Events

<div class="mt-4">

### The Power of CDC for Domain Events

- **Automatic Event Publishing** - Database changes become events automatically
- **Guaranteed Consistency** - Events only published on successful commit
- **Zero Code Changes** - Existing applications work unchanged
- **Support for Patterns** - Enables CQRS, Saga, Outbox natively

</div>

<div class="mt-6">


<img src="/debezium-support-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="Debezium Support Architecture" />

</div>

<!--
Here's where Debezium transforms our architecture. Database changes automatically become domain events. When an order is created, Debezium captures that insert and publishes it to Kafka as an OrderCreated event. Because it reads from the transaction log, events are only published after the transaction commits—giving us perfect consistency. This foundation enables all the advanced patterns we'll explore next.
-->

---
layout: default
---

# Outbox Pattern

**Reliable Event Publishing with Transactional Consistency**

<div class="grid grid-cols-2 gap-6 mt-4">
<div>

### The Problem
- Publishing events and saving data are separate operations
- Risk of data saved but event not published (or vice versa)

### The Solution
- Debezium captures outbox entries
- Events published reliably

### Guarantees
- **Atomicity** - Event and data saved together
- **At-least-once delivery**
- **No message loss**

</div>
<div>

<img src="/outbox-pattern-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="Outbox Pattern" />

<div class="text-xs mt-2 opacity-75">
Outbox table ensures event publishing is part of the transaction
</div>

</div>
</div>

<!--
The Outbox Pattern solves a critical problem: how do you reliably publish events when saving data? If you save to the database and then publish to Kafka, what happens if Kafka is down? You lose the event. If you publish first then save, what if the database save fails? With the Outbox Pattern, you write both your business data and an event record to an outbox table in the same transaction. Debezium monitors the outbox table and publishes events to Kafka. This guarantees that every committed transaction results in a published event.
-->


---
layout: default
---

# CQRS Pattern

**Command Query Responsibility Segregation**

<div class="grid grid-cols-2 gap-6 mt-4">
<div>

### Key Concepts
- **Separate Write and Read Models** - Optimize independently
- **Denormalized Read Data** - Fast queries
- **Event-Driven Updates** - Write model changes propagate to read model
- **Scalability** - Scale reads and writes independently

### Benefits with Debezium


- Automatic synchronization
- No dual-write problem
- Eventual consistency guaranteed



</div>
<div>

<img src="/cqrs-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="CQRS Pattern" />

<div class="text-xs mt-2 opacity-75">
Write model → Debezium → Optimized read models
</div>

</div>
</div>

<!--
CQRS separates write operations from read operations, allowing each to be optimized independently. Your write model can be highly normalized for data integrity, while your read model is denormalized for performance. Debezium bridges the gap: writes go to the normalized database, Debezium captures those changes, and a projection service updates the denormalized read database. This avoids the dual-write problem where you'd have to update both databases in your application code.
-->


---
layout: default
---

# Saga Pattern

**Managing Distributed Transactions**

<div class="grid grid-cols-2 gap-6 mt-4">
<div>

### Distributed Transaction Challenge
- No global transactions across services
- Need coordinated multi-step workflows

### Saga Solution
- Break into local transactions
- Each step publishes events
- Compensating actions for rollback

### Debezium's Role
- Captures state changes reliably
- Triggers next saga steps
- Enables compensation events

</div>
<div>

<img src="/saga-diagram.png" class="w-full h-auto rounded-lg shadow-lg border border-slate-200" alt="Saga Pattern" />

<div class="text-xs mt-2 opacity-75">
Multi-step workflow with compensating actions
</div>

</div>
</div>

<!--
The Saga Pattern manages distributed transactions across multiple services. Instead of a single ACID transaction, a saga is a sequence of local transactions. When an order is created, we start a saga: reserve inventory, process payment, ship order. If payment fails, we need to compensate by releasing the inventory reservation and canceling the order. Debezium captures each state change and triggers the next step. This creates a reliable choreography where services react to events without tight coupling.
-->

---
layout: center
class: text-center
---

# Questions?

<div class="mt-12">

### Resources

🔗 [github.com/debezium/debezium](https://github.com/khaleeeed/dotnet-debezium-rabbitmq-cdc-demo)

📚 [debezium.io/documentation](https://debezium.io/documentation)

</div>

<div class="mt-12 text-sm opacity-75">
Thank you for attending!
</div>

<!--
I'd be happy to answer any questions about implementing these patterns, specific technical details about Debezium, or how to apply these concepts to your own architecture challenges.
-->
