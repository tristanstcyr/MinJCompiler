		% Top of stack: r1
		% Global vars: r2
		% Accumulator 1: r3
		% Accumulator 2: r4
		% Current Frame size: r10
		% Parameter passing: r11
		% helper: r13
		% Return value: r14
		% Return address: r15
		%  
		% Compiler.Tac.Instruction
	entry 
	addi r1,r0,st	% Initialize top stack address
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,0
	lw r3,cs(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L0
		% Compiler.Tac.Instruction
	hlt 
L0	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,0
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,4
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L2
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,8
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,12
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,16
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,20
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,24
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
L5	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L1	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,4
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,28
	lw r3,cs(r3)
	add r14,r0,r3
	j L6	% Compiler.Tac.Instruction+Goto
L6	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L2	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,8
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,32
	lw r3,cs(r3)
	add r14,r0,r3
	j L7	% Compiler.Tac.Instruction+Goto
L7	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L3	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,12
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,36
	lw r3,cs(r3)
	add r14,r0,r3
	j L8	% Compiler.Tac.Instruction+Goto
L8	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L4	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,16
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,40
	lw r3,cs(r3)
	add r14,r0,r3
	j L9	% Compiler.Tac.Instruction+Goto
L9	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
fz	dw 12
	dw 12
	dw 16
	dw 20
	dw 24
cs	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
	dw 0
gb	res 0
st	res 10000	% The stack
